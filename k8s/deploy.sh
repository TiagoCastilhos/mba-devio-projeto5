SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

if [ -z "$IMAGE_TAG" ]; then
  echo "A variável IMAGE_TAG deve ser informada. Exemplo: IMAGE_TAG=v1.0.0 ./deploy.sh"
  exit 1
fi

render_and_apply() {
  envsubst '$IMAGE_TAG' < "$1" | kubectl apply -f -
}

kubectl apply -f "$SCRIPT_DIR/namespace.yaml"
kubectl apply -f "$SCRIPT_DIR/configmap.yaml"
kubectl apply -f "$SCRIPT_DIR/secrets.yaml"
render_and_apply "$SCRIPT_DIR/db.yaml"
kubectl apply -f "$SCRIPT_DIR/rabbitmq.yaml"
render_and_apply "$SCRIPT_DIR/auth.yaml"
render_and_apply "$SCRIPT_DIR/cursos.yaml"
render_and_apply "$SCRIPT_DIR/alunos.yaml"
render_and_apply "$SCRIPT_DIR/pagamentos.yaml"
render_and_apply "$SCRIPT_DIR/bff.yaml"


kubectl rollout status deployment/db -n coldmart --timeout=180s
kubectl rollout status deployment/rabbitmq -n coldmart --timeout=120s

kubectl rollout status deployment/auth -n coldmart --timeout=120s
kubectl rollout status deployment/cursos -n coldmart --timeout=120s
kubectl rollout status deployment/alunos -n coldmart --timeout=120s
kubectl rollout status deployment/pagamentos -n coldmart --timeout=120s
kubectl rollout status deployment/bff -n coldmart --timeout=120s
