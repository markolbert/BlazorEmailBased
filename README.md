# BlazorEmailBased

a Razor class library supporting sites that emails users to grant access

## Background

I had a need for a couple of websites which would only be accessed by a small
number of friends. It's been many years since I last built a website in C#, so
I thought it would be a good time to try out Blazor.

Boy, have things changed! On many fronts, and not just because Blazor is
incredibly powerful (although a bit odd from a console/WinUI C# developer's
perspective). For one thing, doing sophisticated things in CSS is much,
much easier than I remember it being.

But let me stop dating myself :).

This library centralizes several capabilities I'm using in the two websites
I built.

## Authentication

Most websites that have user accounts require a fair bit of plumbing to
do authentication and account management correctly. Much of that is built
automagically by Visual Studio when you create a Blazor app. But my project
was, in part, a learning exercise, I wanted to implement the plumbing myself.

I'd also been intrigued by a growing number of sites that eschew passwords
and passkeys and rely instead on texting or emailing a site access link to
the user's registered email account. In effect, they put the burden of
authentication plumbing on the user's possession of a smart phone or email
account. Since some of my users are very inexperienced with websites, I
decided to go that way. The plumbing to handle this kind of authentication
is part of this library.

## Where Are the Sites Hosted?

To reduce my learning curve I opted to handle everything server side
(although there are a few instances of confirming actions that get
handled via server-side controlled Javascript).

I also wanted to host the sites on a VPS I operate which runs Debian,
currently version Trixie. The need to operate within two different
OSes -- Debian Trixie for production and Windows 11 for development --
made for some interesting requirements. And also several PowerShell scripts
to automate keeping the development and production environment
synchronized.

*I hope you find this project and the related code bases I'll discuss
useful. While I doubt you'll want to use this package directly, I hope it,
and the accompanying documentation will prove useful in coming down your
own learning curve.*
