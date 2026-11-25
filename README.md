# BlazorEmailBased

A Razor class library supporting **Blazor server apps** that email users
to grant access

I hope you find this project and the related code bases I'll discuss
useful. While I doubt you'll want to use this package directly, I hope it,
and the accompanying documentation, will prove useful in coming down your
own learning curve.

Some [background](docs/background.md) on why I wrote this library.

Those portions of the API which directly support a site (e.g.,
`ProgramSupport`) were designed to support **Blazor server apps**. I
don't know if they would work with a Blazor client-side app.

Other parts of the API (e.g., components) should work in all kinds of
Blazor apps...but I've only tested and used them in Blazor server apps.

- Lessons Learned
  - [On the Use of AI](docs/lessons-learned/using-ai.md)
- API
  - [Calendar Component](docs/api/calendar.md)
