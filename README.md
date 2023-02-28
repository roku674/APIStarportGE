# APIStarportGE
//Created by Alexander Fields 

It's an API for StarportGE Data what else did you expect?
https://www.starportgame.com/

<ul>
  <li><a href="https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json/7.0.0">Microsoft.Extensions.Configuration.Json</a></li>
  <li><a href="https://www.nuget.org/packages/Newtonsoft.Json/13.0.2">Newtonsoft.Json</a></li>
  <li><a href="https://www.nuget.org/packages/MongoDB.Bson/2.19.0">MongoDB.Bson</a></li>
  <li><a href="https://www.nuget.org/packages/MongoDB.Driver/2.19.0">MongoDB.Driver</a></li>
  <li><a href="https://www.nuget.org/packages/MongoDB.Driver.Core/2.19.0">MongoDB.Driver.Core</a></li>
  <li><a href="https://www.nuget.org/packages/Swashbuckle.AspNetCore/6.4.0/">Swashbuckle.AspNetCore</a></li>
  <li><a href="https://www.nuget.org/packages/System.Data.DataSetExtensions/4.5.0">System.Data.DataSetExtensions
</ul>

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

Hittin any other endpoint will require the APIKey but you can hit these endpoints without the key <br></br>
http://164.92.72.200/logging/ping <br></br>
http://164.92.72.200/logging/logs (these logs update clear out hourly so if there's nothing thats why) <br></br>
