-> lvlpoc_leader_startconvo


== lvlpoc_leader_startconvo ==
Sorry about all that, been a while since we've had any visitors.
Can I get you something to drink? Some tea? I think we've still got some from the last supply run... yep! 6 months past its use-by, that alright?
* [No, thanks]
     Please, don't be shy! You're our guest, of course you can have the best we have to offer! <>->tea 
        
* [Yes, please!]
    -> tea

= tea
Now, do you take your tea with or without milk?
    * [With]
        Ah, sorry about this, but we don't actually have any...
        -> end_conversation
    * [Without]
        Perfect, because we don't have any.
        -> end_conversation
        
= end_conversation
Anyway, safe travels!
-> DONE