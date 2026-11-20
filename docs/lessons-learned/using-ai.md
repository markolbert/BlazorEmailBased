# On the Use of AI

This project marked my first major use of AI in coding. As with anything,
there's a learning curve. I certainly don't claim to be an expert now,
but here are a few things I've picked up:

All modern AI interfaces display an important disclaimer: they make
mistakes. What's worse, when you're learning something new to you you're
unlikely to sense the mistakes...**because all AIs default to sounding
very, very confident**.

If an AI suggestion doesn't work, *immediately* call the AI on the carpet.
Frankly, depending on how frustrated I am, I often take it out on the AI
in ways I would never talk to a person.

But an even quicker way is simply to come back with **"You're hallucinating
again."** That almost always causes the AI to "reflect" on what its told you,
which often triggers a better response.

How you phrase questions is also important. If you simply
ask "how do I do X?", the AI, depending upon your exact word choice, may
"conclude" you know more than you do, and **give you an answer that you
won't understand**. Even if that doesn't happen, the answer often will not
contain much context...which inhibits your learning.

A better way is to pose questions like **"Tell me how to do X, and explain
why the answer is recommended|works|best practice."** That tends to put the
AI in more of a "teacher mode", which, if you're in a new-to-you area, is
what you want.

Establishing context is also important. For example, I learned the hard way
not ask questions like "In linux, how do I do X?" because there are so many
Linux variations that it's a crapshoot if the answer you get applies to
your particular distribution. So I always ask things like **"In Debian Trixie,
how do I do X, and explain why that works?"**

When setting context sometimes you **really** have to get into the weeds.
I recently had need to configure postfix and dovecot on a Debian Trixie VPS
I operate...and, unbeknownest to me, the latest version of dovecot, which
I was configuring, had made major breaking changes in its configuration
syntax.

I can't tell you how many times I got "answers" that failed utterly because
of those syntax changes. The solution? Always specify the dovecot version:
**"In dovecot 2.4.1 on Debian Trixie, how do I do X, and explain why that's
best practice."**