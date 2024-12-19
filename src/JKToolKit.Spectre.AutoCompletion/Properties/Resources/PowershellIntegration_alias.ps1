# function wpls {
#     if (-not (Get-Command wpcli -ErrorAction SilentlyContinue)) {
#         Write-Error "wpcli is not installed or not found in PATH."
#         return
#     }

#     Invoke-Expression "wpcli ls $args"
# }

# Template:
function [ALIAS_NAME] {
    if (-not (Get-Command [INVOKE_NAME] -ErrorAction SilentlyContinue)) {
        Write-Error "[INVOKE_NAME] is not installed or not found in PATH."
        return
    }

    Invoke-Expression "[INVOKE_NAME] [PARAMS] $args"
}

# Template strings:
# [ALIAS_NAME] - The name of the alias you want to create. (e.g. wpls)
# [INVOKE_NAME] - The name of the command you want to invoke. (e.g. wpcli)
# [PARAMS] - Any additional parameters you want to pass to the command. (e.g. ls)