FROM mcr.microsoft.com/dotnet/sdk:7.0.203-alpine3.17 AS builder
RUN apk add -u make
COPY ./ /build
WORKDIR /build
RUN make publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0.5-alpine3.17
ENV PORT=7777
VOLUME /books
ENV CALIBRELIBRARYPATH='/books'
COPY --from=builder /build/dist /app
WORKDIR /app
ENTRYPOINT "./bookbrowser"