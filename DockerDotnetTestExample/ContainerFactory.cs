using System;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace DockerDotnetTestExample
{
    public static class ContainerFactory
    {
        private const string UnixDockerEndpoint = "unix:///var/run/docker.sock";

        private const string WindowsDockerEndpoint = "npipe://./pipe/docker_engine";

        private const string Image = "mcr.microsoft.com/mssql/server";

        private const string Tag = "2019-latest";

        private static readonly string FullImage = $"{Image}:{Tag}";

        private static DockerClient GetClient() => new DockerClientConfiguration(new Uri(UnixDockerEndpoint)).CreateClient();

        public static async Task<string> StartContainer()
        {
            using(var client = GetClient())
            {
                var images = await client
                    .Images
                    .ListImagesAsync(new ImagesListParameters(){ MatchName = FullImage });

                if(images.Count == 0)
                {
                    await client
                        .Images
                        .CreateImageAsync(
                            new ImagesCreateParameters { Tag = Tag, FromImage = Image },
                            null,
                            new Progress<JSONMessage>());
                }

                await StopContainer(TestProperties.ContainerName);

                var container = await client.Containers.CreateContainerAsync(
                    new CreateContainerParameters()
                    {
                        Name = TestProperties.ContainerName,
                        Env = new List<string>()
                        {
                            "ACCEPT_EULA=Y",
                            $"SA_PASSWORD={TestProperties.Password}",
                        },
                        ExposedPorts = new Dictionary<string, EmptyStruct>()
                        {
                            ["1433"] = default,
                        },
                        HostConfig = new HostConfig()
                        {
                            PortBindings = new Dictionary<string, IList<PortBinding>>()
                            {
                                ["1433"] = new List<PortBinding>()
                                    { new PortBinding() { HostIP = "localhost", HostPort = TestProperties.Port } },
                            },
                        },
                        Image = FullImage,
                    });

                var successfullyStarted = await client
                    .Containers
                    .StartContainerAsync(
                        container.ID, 
                        new ContainerStartParameters() {DetachKeys = $"d={FullImage}"});

                if(!successfullyStarted)
                {
                    throw new Exception("Could not start container");
                }

                //Need this because container is not immediately available
                await Task.Delay(10_000);

                return container.ID;
            }
        }

        private static async Task StopContainer(string containerName)
        {
            using(var client = GetClient())
            {
                var containers = await client.Containers.ListContainersAsync(new ContainersListParameters());

                var existingContainer = containers.FirstOrDefault(x => x.Names.Contains("/" + containerName));

                if(existingContainer == null)
                {
                    return;
                }

                if(await client.Containers.StopContainerAsync(existingContainer.ID, new ContainerStopParameters()))
                {
                    await client.Containers.RemoveContainerAsync(existingContainer.ID, new ContainerRemoveParameters());
                }
            }
        }
    }
}