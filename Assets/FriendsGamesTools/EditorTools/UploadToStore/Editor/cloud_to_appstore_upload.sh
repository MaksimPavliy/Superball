#!/bin/bash
 
echo "Uploading IPA to Appstore Connect..."
 
#Path is "$WORKSPACE/.build/last/<BUILD_TARGET_ID>/build.ipa"
path="$WORKSPACE/.build/last/ios-appstore/build.ipa"
 
if xcrun altool --upload-app --type ios -f $path -u "dskyfriendsgames@gmail.com" -p "gyxb-nsyk-evym-jnkm" --asc-provider 77HBQK6TTN; then
    echo "Upload IPA to Appstore Connect finished with success"
else
    echo "Upload IPA to Appstore Connect failed"
fi
