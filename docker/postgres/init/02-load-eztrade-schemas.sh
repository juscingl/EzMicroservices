#!/usr/bin/env bash
# 由 postgres 官方镜像在首次初始化数据目录时执行（与 01-create-databases.sql 同目录，按文件名顺序在 01 之后）。
# 将 deploy/sql 下各库脚本分别执行到对应 database（init 阶段默认连接的是 POSTGRES_DB，不能直接替代多库脚本）。
set -euo pipefail

SQL_DIR="${EZTRADE_DEPLOY_SQL_DIR:-/eztrade-deploy-sql}"

if [[ ! -d "$SQL_DIR" ]]; then
  echo "ERROR: SQL 目录不存在: $SQL_DIR（请在 compose 中挂载 deploy/sql 到该路径）" >&2
  exit 1
fi

run_sql() {
  local db_name="$1"
  local file_name="$2"
  local file_path="${SQL_DIR}/${file_name}"
  if [[ ! -f "$file_path" ]]; then
    echo "ERROR: 找不到 SQL 文件: $file_path" >&2
    exit 1
  fi
  echo "==> 初始化库 ${db_name} <- ${file_name}"
  psql -v ON_ERROR_STOP=1 --username "${POSTGRES_USER}" --dbname "${db_name}" -f "${file_path}"
}

run_sql authdb authcenter-init.sql
run_sql ordersdb orders-init.sql
run_sql inventorydb inventory-init.sql
run_sql paymentsdb payments-init.sql

echo "==> 多库 schema 初始化完成"
