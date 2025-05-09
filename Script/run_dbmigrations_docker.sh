#!/bin/bash

[[ -n $1 ]] || { echo "Usage: $0 <Docker network>"; exit 1; }

declare -r docker_image="dsp8ef9/bookprices_jobapi_dbmigration:latest"
declare -r docker_network="$1" # Network should already exist and be attachable

docker pull $docker_image \
    && docker run "$docker_image" --network="$docker_network" --env-file dbmigration.env

[[ $? -eq 0 ]] || { echo "Migration failed"; exit 1; }
