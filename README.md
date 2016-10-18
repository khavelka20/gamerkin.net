#Gamer Kin .Net Version

## Summary
This solution is only running on my local machine.

This is an unfinished project I started about 1 year ago. 

It is a port of the Laravel project. I learned to really love C# and visual studio so I wanted to move this project from PHP to C#.

I wanted to develop a tool that would predict games a customer would like based upon games they have played, and show them a relevant video clip of a game without having to manually generate the assets.

I developed a web solution that allows customers to pull in their gaming history from a common gaming service (Steam). The site then shows the customer games they are likely to buy based upon their utility matrix. 

I also automated the asset generation by creating a tool that searches for relevant video assets, and creates short clips from the assets using FFMpeg.

## Stack Used

<ul>
<li>Server Side
<ul>
<li>ASP.Net WebApi and MVC5</li>
<li>MySQL (Using a Microsoft Connector)</li>
<li>Entity Framework, Automapper, Unity</li>
<li>FFMPEG</li>
</ul>
</li>
<li>Client Side
<ul>
<li>HTML 5</li>
<li>CSS 3</li>
<li>AngularJS</li>
</ul>
</li>

</ul>
