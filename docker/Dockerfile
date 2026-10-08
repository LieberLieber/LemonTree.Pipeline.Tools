FROM debian:bookworm-slim

RUN apt-get update \
    && apt-get install --no-install-recommends -y libicu72 libssl3 zlib1g \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY LemonTree.Pipeline.Tools.ModelCheck ./LemonTree.Pipeline.Tools.ModelCheck
COPY checks-config.json ./checks-config.json
RUN chmod +x ./LemonTree.Pipeline.Tools.ModelCheck

ENTRYPOINT ["./LemonTree.Pipeline.Tools.ModelCheck"]
