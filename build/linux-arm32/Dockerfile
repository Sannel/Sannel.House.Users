ARG tag=3.0
FROM mcr.microsoft.com/dotnet/core/aspnet:${tag} AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443
COPY qemu-arm-static /usr/bin/
COPY Sannel.House.Users/ .
RUN ln -s  /var/run/secrets/kubernetes.io/serviceaccount/ca.crt /etc/ssl/certs/kubernetes.pem \
    && addgroup -g 1200 house \
    && adduser -u 1100 -G 1200 house \
    && chown -R house:house /app \
    && rm -f /usr/bin/qemu-arm-static
USER 1200
ENTRYPOINT ["dotnet", "Sannel.House.Users.dll"]