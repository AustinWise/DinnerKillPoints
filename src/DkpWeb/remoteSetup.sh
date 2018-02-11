#!/bin/sh
set -e
set -x

OLDDIR=oldpublish
CURDIR=publish
TARNAME=publish.tar

systemctl stop dkp

if [ -d $OLDDIR ]
then
  rm -r $OLDDIR
fi

if [ -d $CURDIR ]
then
  mv $CURDIR $OLDDIR
fi

tar xf $TARNAME
chmod +x $CURDIR/DkpWeb

systemctl start dkp
