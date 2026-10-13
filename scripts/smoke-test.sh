#!/usr/bin/env bash
# Smoke test pós-deploy: aguarda a nova versão responder e o /health ficar "Healthy".
# Uso: scripts/smoke-test.sh <url-da-aplicacao> [versao-esperada]
set -uo pipefail

URL="${1%/}"
VERSAO_ESPERADA="${2:-}"
TENTATIVAS=36   # 36 x 10s = até 6 minutos (tempo de pull e start do contêiner no Azure)

echo "🔎 Smoke test em: $URL"
for i in $(seq 1 "$TENTATIVAS"); do
  STATUS=$(curl -s -o /tmp/health.json -w "%{http_code}" "$URL/health" || echo "000")
  VERSAO=$(curl -s "$URL/" | grep -o '"versao":"[^"]*"' | cut -d'"' -f4 || true)

  if [ "$STATUS" = "200" ] && { [ -z "$VERSAO_ESPERADA" ] || [ "$VERSAO" = "$VERSAO_ESPERADA" ]; }; then
    echo "✅ Aplicação saudável (HTTP $STATUS) — versão: $VERSAO"
    cat /tmp/health.json; echo
    curl -s "$URL/"; echo
    exit 0
  fi

  echo "⏳ Tentativa $i/$TENTATIVAS — HTTP $STATUS, versão em execução: '${VERSAO:-?}'. Aguardando 10s..."
  sleep 10
done

echo "❌ A aplicação não respondeu corretamente após $TENTATIVAS tentativas."
exit 1
