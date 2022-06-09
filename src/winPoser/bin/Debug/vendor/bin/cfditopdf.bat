@ECHO OFF
setlocal DISABLEDELAYEDEXPANSION
SET BIN_TARGET=%~dp0/../phpcfdi/cfditopdf/bin/cfditopdf
php "%BIN_TARGET%" %*
