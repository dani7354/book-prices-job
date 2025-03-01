#!/bin/bash
#
# Apply migrations from both database contexts, used for the Docker dbmigration service
#

declare -r ef_project="/src/BookPricesJob.Data"
declare -r startup_project="/src/BookPricesJob.API"


echo "Applying changes from DatabaseContextMysql..."
dotnet-ef database update --project "$ef_project" --startup-project "$startup_project" --context DefaultDatabaseContext
if [[ "$?" -ne "0" ]] ; then
    echo "Failed to apply changes from DefaultDatabaseContext!" >&2
    exit 1
fi

