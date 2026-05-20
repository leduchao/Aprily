using Aprily.Application.Abstractions.Cqrs;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

namespace Aprily.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        // Add license key for MediatR and AutoMapper (free license)
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(typeof(DependencyInjection).Assembly);
            config.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODEwNzcxMjAwIiwiaWF0IjoiMTc3OTI3MDI3NSIsImFjY291bnRfaWQiOiIwMTllNDRjNTI2YzU3ODA5YTNkZWI3ZTJiMDJhNDJkYiIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa3MyY2JtcmJrZGUxYnlxM3B2N2UzanRrIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.l5Aa9SJDQIRL1LkM4k8FRs_Lks8NOYA42NH8nYPia3SFu7102xazEsDMMOcXR3aPTq44F_XM3ZRdr7TVfHJDT9CpbqKVdjLE_hJPeiVpoQ4f6Q9r5U0YUyZbrDP6Af0DyksaFdE84C370DuqLoF7wx5rYDKrIGQNQWQ6QFTPMPnguAbgVv35ft0WI8y5XwB_AX0QzHlTmfLN4j_ueuChaQb_AhahsVENtSQyQjd0NnSKHkRFOM-jCrEhyyFGqa1xj7OVMv4mtSgyMTUOiQIDOMLRlvJ2Hca_DOWLkEPNG9Y5DJysvRp8zCwALdH_-WYN0is___FQE32p5tZODbTkHg";
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }
}
