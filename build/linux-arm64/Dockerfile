ARG tag=3.0
ARG tagCodeName=alpine
FROM mcr.microsoft.com/dotnet/core/aspnet:${tag}-${tagCodeName}-arm64v8 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443
COPY qemu-aarch64-static /usr/bin/
COPY Sannel.House.Users/ .
RUN ln -s  /var/run/secrets/kubernetes.io/serviceaccount/ca.crt /etc/ssl/certs/kubernetes.pem \
    && addgroup -g 1200 house \
    && adduser -u 1200 -G house -D house \
    && chown -R house:house /app \
    && rm -f /usr/bin/qemu-aarch64-static
USER 1200
ENTRYPOINT ["dotnet", "Sannel.House.Users.dll"]