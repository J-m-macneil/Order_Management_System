#!/bin/sh
set -eu

API_BASE_URL="${API_BASE_URL:-http://localhost:8080/api}"

cat > /usr/share/nginx/html/runtime-config.js <<EOF
window.__backConfig = {
  apiBaseUrl: '${API_BASE_URL}'
};
EOF

exec nginx -g 'daemon off;'
