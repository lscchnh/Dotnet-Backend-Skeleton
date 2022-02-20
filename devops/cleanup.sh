kubectl delete deployment webapi
kubectl delete service webapi
docker container rm -f $(docker ps -aq)
docker image rm backendskeleton:dev