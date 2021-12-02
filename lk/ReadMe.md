# lk
lk is a CLI to set a lock with key, token and expired tiem
`lk [OPERATE] [lock key] [lock token]`

## LOCK
`lk LOCK mylock mytoken 1000`

## RELEASE
`lk RELEASE mylock mytoken`

## Get the return code by windows command line
```
.\lk.exe LOCK mylock1 lock1 10000
echo Exit Code is %errorlevel%
```