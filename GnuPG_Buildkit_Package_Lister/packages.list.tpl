#!/bin/bash

set -e
set -o pipefail

gpg_verify="gpg --homedir $PWD/.gnupg -d"

package_list=("libgpg-error" "libksba" "libgcrypt" "libassuan" "npth" "gnupg" "pinentry")
package_version=("<libgpg-error>" "<libksba>" "<libgcrypt>" "<libassuan>" "<npth>" "<gnupg>" "<pinentry>")
