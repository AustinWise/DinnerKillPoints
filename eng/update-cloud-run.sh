#!/usr/bin/env bash

set -e


gcloud run services replace \
    --project=dinnerkillpoints \
    --region=us-central1 \
    cloudrun.yaml
