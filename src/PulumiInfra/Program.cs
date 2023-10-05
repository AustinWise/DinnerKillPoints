using Pulumi;
using Pulumi.Gcp.CloudRun;
using Pulumi.Gcp.CloudRun.Inputs;
using System.Collections.Generic;
using System.Linq;

const string LOCATION = "us-central1";

return await Deployment.RunAsync(() =>
{
    // TODO: build docker image

    var service = new Service("dkptest", new ServiceArgs()
    {
        Location = LOCATION,
        Metadata = new ServiceMetadataArgs()
        {
            Annotations = new Dictionary<string, string>()
            {
                { "run.googleapis.com/ingress", "all" },
                { "run.googleapis.com/launch-stage", "BETA"},
            }
        },
        Template = new ServiceTemplateArgs()
        {
            Metadata = new ServiceTemplateMetadataArgs()
            {
                Annotations = new Dictionary<string, string>()
                {
                    { "run.googleapis.com/execution-environment", "gen2" },
                    { "run.googleapis.com/startup-cpu-boost", "true" },
                }
            },
            Spec = new ServiceTemplateSpecArgs()
            {
                ContainerConcurrency = 80,
                ServiceAccountName = "dkp-service-account@dinnerkillpoints.iam.gserviceaccount.com",
                Containers = new List<ServiceTemplateSpecContainerArgs>()
                {
                    new ServiceTemplateSpecContainerArgs()
                    {
                        // TODO: use built image
                        Name = "dkpweb",
                        Image = "us-central1-docker.pkg.dev/dinnerkillpoints/cloud-run-source-deploy/dinnerkillpoints/dkpweb:38e51ad9d5067b0d9f0e054399ad0fe6db8a4af2",
                        Ports = new List<ServiceTemplateSpecContainerPortArgs>()
                        {
                            new ServiceTemplateSpecContainerPortArgs()
                            {
                                Name = "http1",
                                ContainerPort = 8080,
                            },
                        },
                        StartupProbe = new ServiceTemplateSpecContainerStartupProbeArgs()
                        {
                            FailureThreshold = 5,
                            PeriodSeconds = 2,
                            TimeoutSeconds = 1,
                            HttpGet = new ServiceTemplateSpecContainerStartupProbeHttpGetArgs()
                            {
                                Path = "/healthz",
                            },
                        },
                        Envs = new List<ServiceTemplateSpecContainerEnvArgs>()
                        {
                            new ServiceTemplateSpecContainerEnvArgs()
                            {
                                Name = "DataProtection__Bucket",
                                Value = "dkp-settings",
                            },
                            new ServiceTemplateSpecContainerEnvArgs()
                            {
                                Name = "DataProtection__ObjectName",
                                Value = "DataProtectionKeys.xml",
                            },
                            new ServiceTemplateSpecContainerEnvArgs()
                            {
                                Name = "GcpProjectId",
                                Value = "216611213396",
                            },
                            new ServiceTemplateSpecContainerEnvArgs()
                            {
                                Name = "ConnectionStrings__Postgres",
                                Value = "host=127.0.0.1; database=dkp; username=dkp-service-account@dinnerkillpoints.iam; password=noop;",
                            },
                        },
                    },
                    new ServiceTemplateSpecContainerArgs()
                    {
                        Image = "us.gcr.io/cloud-sql-connectors/cloud-sql-proxy:2.7.0",
                        Name = "sql",
                        Args = new List<string>()
                        {
                            "--structured-logs",
                            "--auto-iam-authn",
                            "--health-check",
                            "--http-address=0.0.0.0",
                            "austinsql:us-central1:austinsql",
                        },
                        StartupProbe = new ServiceTemplateSpecContainerStartupProbeArgs()
                        {
                            FailureThreshold = 5,
                            PeriodSeconds = 2,
                            TimeoutSeconds = 1,
                            HttpGet = new ServiceTemplateSpecContainerStartupProbeHttpGetArgs()
                            {
                                Path = "/startup",
                                Port = 9090,
                            },
                        },
                    },
                },
            },
        },
    });

    var serviceBinding = new IamBinding("dkptestiambinding", new IamBindingArgs()
    {
        Role = "roles/run.invoker",
        Members = new List<string>() { "allUsers" },
        Service = service.Name,
        Location = LOCATION,
    });

    return new Dictionary<string, object?>
    {
        ["service_url"] = service.Statuses.Apply(o => o.Select(s => s.Url).First()),
    };
});
