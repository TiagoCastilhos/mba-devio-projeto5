#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

kubectl delete -f "$SCRIPT_DIR/bff.yaml" --ignore-not-found=true
kubectl delete -f "$SCRIPT_DIR/pagamentos.yaml" --ignore-not-found=true
kubectl delete -f "$SCRIPT_DIR/alunos.yaml" --ignore-not-found=true
kubectl delete -f "$SCRIPT_DIR/cursos.yaml" --ignore-not-found=true
kubectl delete -f "$SCRIPT_DIR/auth.yaml" --ignore-not-found=true
kubectl delete -f "$SCRIPT_DIR/rabbitmq.yaml" --ignore-not-found=true
kubectl delete -f "$SCRIPT_DIR/db.yaml" --ignore-not-found=true
kubectl delete -f "$SCRIPT_DIR/secrets.yaml" --ignore-not-found=true
kubectl delete -f "$SCRIPT_DIR/namespace.yaml" --ignore-not-found=true

kubectl delete namespace coldmart --ignore-not-found=true
