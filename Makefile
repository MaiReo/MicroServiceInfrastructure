DOTNET_CLI := dotnet

all: build

build:
	$(DOTNET_CLI) build

pack:
	$(DOTNET_CLI) pack -c Release

clean:
	$(DOTNET_CLI) clean;
	$(DOTNET_CLI) clean -c Release;

.PHONY: all build pack clean
