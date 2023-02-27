# APILogging
//Created by Alexander Fields 

It's an API for Starport Data what else did you expect?

<ul>
  <li><a href="https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json/7.0.0">Microsoft.Extensions.Configuration.Json</a></li>
  <li><a href="https://www.nuget.org/packages/Microsoft.VisualStudio.Azure.Containers.Tools.Targets/1.17.0/">Microsoft.VisualStudio.Azure.Containers.Tools.Targets</a></li>
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

