SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

kubectl apply -f "$SCRIPT_DIR/namespace.yaml"
kubectl apply -f "$SCRIPT_DIR/secrets.yaml"
kubectl apply -f "$SCRIPT_DIR/db.yaml"
kubectl apply -f "$SCRIPT_DIR/rabbitmq.yaml"
kubectl apply -f "$SCRIPT_DIR/auth.yaml"
kubectl apply -f "$SCRIPT_DIR/cursos.yaml"
kubectl apply -f "$SCRIPT_DIR/alunos.yaml"
kubectl apply -f "$SCRIPT_DIR/pagamentos.yaml"
kubectl apply -f "$SCRIPT_DIR/bff.yaml"


kubectl rollout status deployment/db -n coldmart --timeout=180s
kubectl rollout status deployment/rabbitmq -n coldmart --timeout=120s

kubectl rollout status deployment/auth -n coldmart --timeout=120s
kubectl rollout status deployment/cursos -n coldmart --timeout=120s
kubectl rollout status deployment/alunos -n coldmart --timeout=120s
kubectl rollout status deployment/pagamentos -n coldmart --timeout=120s
kubectl rollout status deployment/bff -n coldmart --timeout=120s
