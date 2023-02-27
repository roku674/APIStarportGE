# APIStarportGE

docker rmi -f $(docker images -aq)
docker build -f "C:\Users\ZANDER\source\repos\APIStarportGE\APIStarportGE\Dockerfile" --force-rm -t roku674/apistarportge  --label "com.microsoft.created-by=visual-studio" --label "com.microsoft.visual-studio.project-name=APIStarportGE" "C:\Users\ZANDER\source\repos\APIStarportGE" 
docker push docker.io/roku674/apistarportge:latest


docker logs $(docker ps -a -q)
docker stop $(docker ps -q)
docker rmi -f $(docker images -aq)
docker rm $(docker ps -a -q)
docker pull roku674/apistarportge:latest
docker run --ip 172.17.0.1 \
-p 164.92.72.200:80:80 \
-p 164.92.72.200:443:443 \
roku674/apistarportge


docker run -d -p 2717:27017 -v ~/mongodb-starportge:/data/db --name mymongo mongo:latest


docker exec -it mymongo bash

Arc 1/5|~{yellow}~Des 0/0|~{green}~Earth 1/1|~{orange}~Green 0/1|~{purple}~Mount 0/0|~{blue}~Oce 1/5|~{pink}~IGPs ~{link}1:0~|~{gray}~Roc 0/0|~{red}~Volc 0/1|~{link}25:Caps:~ 13|~{green}~DDs: 0|~{cyan}~4 Zounds/13~{link}21: Cols~
