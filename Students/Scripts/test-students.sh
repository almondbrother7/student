#!/usr/bin/env bash
# -----------------------------------------------------------------------------
# Students API smoke test
#
# HOW TO RUN:
#   1) Start the API (in-memory example):
#        STUDENTS_REPO=memory dotnet run
#      or if you use launch profiles:
#        dotnet run --launch-profile "Students (InMemory)"
#
#   2) Make this script executable and run it:
#        chmod +x test-students.sh
#        bash ./Scripts/test-students.sh
#
#      Options:
#        BASE=http://localhost:5069 ./test-students.sh   # use HTTP instead of HTTPS
#        INSECURE=false ./test-students.sh               # don't use -k for TLS (if certs are trusted)
#
#   Tip: Install 'jq' to pretty-print JSON (brew install jq).
# -----------------------------------------------------------------------------

set -euo pipefail

BASE="${BASE:-https://localhost:7062}"
API="$BASE/api/students"
CURL_K=""
if [[ "${INSECURE:-true}" == "true" ]] && [[ "$BASE" == https://* ]]; then
  CURL_K="-k"
fi

pretty() { if command -v jq >/dev/null 2>&1; then jq .; else cat; fi; }
say() { printf "\n==> %s\n" "$*"; }

echo "Using API: $API"

# CREATE
say "POST create a new student (server assigns id)"
CREATE_BODY='{
  "firstName": "Test",
  "lastName": "Acall",
  "address": "1 Test Ave, Springfield, IL 62701",
  "dateOfBirth": "2008-12-28",
  "email": "test@example.com",
  "phone": "321-555-0000",
  "grade": "10"
}'
CREATE_RESP=$(curl -sS $CURL_K -H "Content-Type: application/json" -X POST "$API" -d "$CREATE_BODY")
echo "$CREATE_RESP" | pretty
if command -v jq >/dev/null 2>&1; then
  ID=$(echo "$CREATE_RESP" | jq -r '.id')
else
  ID=$(echo "$CREATE_RESP" | sed -n 's/.*"id"[[:space:]]*:[[:space:]]*\([0-9][0-9]*\).*/\1/p')
fi
[[ -n "${ID:-}" && "$ID" != "null" ]] || { echo "!! Could not parse created student Id"; exit 1; }
echo "Created student Id: $ID"

# GET ALL
say "GET all students"
curl -sS $CURL_K "$API" | pretty

# GET BY ID
say "GET student by id ($ID)"
curl -sS $CURL_K "$API/$ID" | pretty

# UPDATE
say "PUT update student ($ID) — route id is the source of truth"
UPDATE_BODY='{
  "firstName": "Test",
  "lastName": "Acall",
  "address": "123 Updated St, Springfield, IL 62701",
  "dateOfBirth": "2008-12-28",
  "email": "test@example.com",
  "phone": "321-555-0000",
  "grade": "11"
}'
UPDATE_CODE=$(curl -sS $CURL_K -o /dev/null -w '%{http_code}' -H "Content-Type: application/json" -X PUT "$API/$ID" -d "$UPDATE_BODY")
echo "HTTP $UPDATE_CODE (expect 204)"

# VERIFY UPDATE
say "GET student by id (verify update)"
curl -sS $CURL_K "$API/$ID" | pretty

# DELETE
say "DELETE student ($ID)"
DEL_CODE=$(curl -sS $CURL_K -o /dev/null -w '%{http_code}' -X DELETE "$API/$ID")
echo "HTTP $DEL_CODE (expect 204)"

# VERIFY DELETE
say "GET student by id after delete (should be 404)"
GET_CODE=$(curl -sS $CURL_K -o /dev/null -w '%{http_code}' "$API/$ID")
echo "HTTP $GET_CODE (expect 404)"

printf "\nDone.\n"
