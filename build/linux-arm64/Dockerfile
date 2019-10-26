ARG tag=3.0
FROM mcr.microsoft.com/dotnet/core/aspnet:${tag} AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443
COPY qemu-aarch64-static /usr/bin/
COPY Sannel.House.Users/ .
RUN ln -s  /var/run/secrets/kubernetes.io/serviceaccount/ca.crt /etc/ssl/certs/kubernetes.pem \
    && groupadd -g 1200 house \
    && useradd -m house -u 1100 -g 1200 \
    && chown -R house:house /app \
    && rm -f /usr/bin/qemu-aarch64-static
USER 1200
ENTRYPOINT ["dotnet", "Sannel.House.Users.dll"]