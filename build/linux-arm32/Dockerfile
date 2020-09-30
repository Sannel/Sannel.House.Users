ARG tag=3.0
ARG tagCodeName=buster-slim
FROM mcr.microsoft.com/dotnet/core/aspnet:${tag}-buster-slim-arm32v7 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443
COPY qemu-arm-static /usr/bin/
COPY Sannel.House.Users/ .
RUN ln -s  /var/run/secrets/kubernetes.io/serviceaccount/ca.crt /etc/ssl/certs/kubernetes.pem \
    && addgroup --gid 1200 house \
    && useradd --uid 1200 --gid house house \
    && chown -R house:house /app \
    && rm -f /usr/bin/qemu-arm-static
USER 1200
ENTRYPOINT ["dotnet", "Sannel.House.Users.dll"]