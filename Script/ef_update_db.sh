#!/bin/bash
#
# Apply migrations from both database contexts, used for the Docker dbmigration service
#

declare -r ef_project="/src/BookPricesJob.Data"
declare -r startup_project="/src/BookPricesJob.API"


echo "Applying changes from DatabaseContextMysql..."
dotnet-ef database update --project "$ef_project" --startup-project "$startup_project" --context DatabaseContextMysql
if [[ "$?" -ne "0" ]] ; then
    echo "Failed to apply changes from DatabaseContextMysql!" >&2
    exit 1
fi

echo "Applying chages from IdentityDatabaseContextMysql..."
dotnet-ef database update --project "$ef_project" --startup-project "$startup_project" --context IdentityDatabaseContextMysql
if [[ "$?" -ne "0" ]] ; then
    echo "Failed to apply changes from IdentityDatabaseContextMysql!" >&2
    exit 1
fi

