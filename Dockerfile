FROM mcr.microsoft.com/dotnet/sdk:7.0.203-alpine3.17 AS builder
WORKDIR /build
COPY ./src/ ./src
COPY ./test ./test
COPY ./*.sln ./
WORKDIR /build
RUN dotnet test
RUN dotnet publish -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0.5-alpine3.17
ENV PORT=7777
VOLUME /books
ENV CALIBRELIBRARYPATH='/books'
COPY --from=builder /app /app
WORKDIR /app
ENTRYPOINT "./bookbrowser"