
test:
	docker run --rm -w /repo -v $$PWD:/repo -t mcr.microsoft.com/dotnet/sdk:7.0.203-alpine3.17 dotnet test

clean:
	dotnet restore
	dotnet clean -c Release

publish: clean
	dotnet publish ./src/bookbrowser.csproj -c Release -o ./dist

containers:
	docker buildx build --platform linux/amd64 --platform linux/arm64 -t atheken/bookbrowser:latest .