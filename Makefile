test:
	docker run --rm -w /repo -v $$PWD:/repo -t mcr.microsoft.com/dotnet/sdk:7.0.203-alpine3.17 dotnet test