#!/usr/bin/env bash
# =====================================================================
# Cria a infraestrutura no Azure (rodar no Azure Cloud Shell - Bash):
#   - Resource Group + App Service Plan Linux
#   - 2 Web Apps para contêiner: staging e produção
#   - Variáveis de ambiente de cada ambiente (banco separado por ambiente)
#   - Gera os publish profiles usados como secrets no GitHub
#
# Uso:
#   bash azure-setup.sh <usuario-github> "<connection-string-mongodb-atlas>"
# Opcional: LOCATION=eastus SKU=B1 bash azure-setup.sh ...
# =====================================================================
set -euo pipefail

if [ $# -lt 2 ]; then
  echo "Uso: bash azure-setup.sh <usuario-github> \"<connection-string-mongodb-atlas>\""
  exit 1
fi

GITHUB_USER=$(echo "$1" | tr '[:upper:]' '[:lower:]')
MONGO_CONN="$2"
SUFIXO="${SUFIXO:-rm557663}"
LOCATION="${LOCATION:-eastus}"
SKU="${SKU:-B1}"

RG="rg-ecocoleta-$SUFIXO"
PLAN="plan-ecocoleta-$SUFIXO"
IMAGE="ghcr.io/$GITHUB_USER/ecocoleta-api:latest"

echo "==> Resource group $RG ($LOCATION)"
az group create --name "$RG" --location "$LOCATION" --output none

echo "==> App Service Plan $PLAN (Linux, $SKU)"
az appservice plan create --name "$PLAN" --resource-group "$RG" --is-linux --sku "$SKU" --output none

for AMB in staging prod; do
  APP="ecocoleta-$AMB-$SUFIXO"
  if [ "$AMB" = "prod" ]; then ASPNET_ENV="Production"; else ASPNET_ENV="Staging"; fi

  echo "==> Web App $APP ($ASPNET_ENV)"
  az webapp create --name "$APP" --resource-group "$RG" --plan "$PLAN" \
    --container-image-name "$IMAGE" --output none

  az webapp config appsettings set --name "$APP" --resource-group "$RG" --output none --settings \
    WEBSITES_PORT=8080 \
    ASPNETCORE_ENVIRONMENT="$ASPNET_ENV" \
    Mongo__ConnectionString="$MONGO_CONN" \
    Mongo__Database="ecocoleta_$AMB" \
    Seed__Enabled=true

  az webapp config set --name "$APP" --resource-group "$RG" --always-on true --output none

  # Habilita credenciais de publicação (necessário para o publish profile do GitHub Actions)
  az resource update --resource-group "$RG" --name scm --namespace Microsoft.Web \
    --resource-type basicPublishingCredentialsPolicies --parent "sites/$APP" \
    --set properties.allow=true --output none

  az webapp deployment list-publishing-profiles --name "$APP" --resource-group "$RG" --xml > "publish-profile-$AMB.xml"
  echo "    URL: https://$(az webapp show --name "$APP" --resource-group "$RG" --query defaultHostName -o tsv)"
done

echo ""
echo "✅ Infraestrutura criada!"
echo "   Publish profiles gerados: publish-profile-staging.xml e publish-profile-prod.xml"
echo "   Veja o conteúdo com:  cat publish-profile-staging.xml"
echo "   Ao final da avaliação, apague tudo com:  az group delete --name $RG --yes"
