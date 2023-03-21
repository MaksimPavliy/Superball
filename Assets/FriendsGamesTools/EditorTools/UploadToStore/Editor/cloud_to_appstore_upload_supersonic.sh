#!/bin/bash
 
echo "Uploading IPA to Appstore Connect..."
 
#Path is "$WORKSPACE/.build/last/<BUILD_TARGET_ID>/build.ipa"
path="$WORKSPACE/.build/last/ios-appstore/build.ipa"
 
if xcrun altool --upload-app --type ios -f $path -u "fgileving@gmail.com" -p "mhxf-nqbb-fick-eicn" --asc-provider 742U9WHP23; then
    echo "Upload IPA to Appstore Connect finished with success"
else
    echo "Upload IPA to Appstore Connect failed"
fi
