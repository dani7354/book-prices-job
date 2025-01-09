#!/bin/bash

[[ ! -x docker ]] || { echo "Docker not installed"; exit 1; }

docker service update --image dsp8ef9/bookprices_jobapi:latest bookprices_job_api

[[ $? -eq 0 ]] || { echo "Service update failed"; exit 1; }
