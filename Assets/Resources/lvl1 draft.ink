VAR did_player_competently_enter_lvl1_village = false
VAR agatha_trust = 0
VAR ralphie_trust = 0
VAR wearing_uniform = true
VAR ralphie_knows_kristas_name = false
VAR paul_trust = 0

/* Prefixes:

SL1Agatha - short for StoryLetAgatha
SL1Ralphie - short for StoryLet[Level]1ralphie
SL1Paul
SL1David
SL1Cassie

*/



== select_entry_type == 
System: Did you succeed at level 1-7's puzzle?
*[Yes]
    ~did_player_competently_enter_lvl1_village = true
*[No]
    ~did_player_competently_enter_lvl1_village = false
- -> enter_lvl1_village


== enter_lvl1_village ==
*{did_player_competently_enter_lvl1_village ==false} -> SL1Agatha_found_by_old_lady
*-> hub_character_hub

== hub_character_hub ==
System: Who will you talk to?
* [Ralphie y] -> SL1Ralphie_talk_to_ralphie
* [Paul n] -> SL1Paul_talk_to_paul 
* [Old lady y] -> SL1Agatha_talk_to_agatha
* [SAlt lady (messenger) n] -> lvl1_time_to_vote
* [Scared guy (doomsday guy) ?]-> lvl1_time_to_vote
* [Drug victim (dead)]-> lvl1_time_to_vote
* ->
    ->lvl1_time_to_vote


== function _SLAgatha_found_by_old_lady ==
~ return (not did_player_competently_enter_lvl1_village)
==SL1Agatha_found_by_old_lady ==
~agatha_trust += 2
???: Hello? You alright, dear?
*[I think so...]
    Great, now <>
*[Who are you?]
    ~agatha_trust -= 1
    Who are YOU? Don't worry about that, <> 
-come on, let's get you warm and dry. Well? What are you waiting for?
... 
Agatha: So, what *cough* brings you down here? It's Agatha, by the way.
* [I got lost.]
    ~agatha_trust -= 2
    ... You know you're still in uniform, right?
    
* [I'm from the Department of Transport.]
    ~agatha_trust += 1
    We're not in trouble, I hope?
    ** [I don't think so...]
        In that case, neither are you! Although it looked like you were before, what happened?
        ***[I misjudged my abilities a bit.]
            ~agatha_trust += 1
            Well, we all do that sometimes, don't we dear? More and *cough* more these days...
        ***[Nothing!]
            ~agatha_trust -= 1
            Alright, alright! No need to get defensive.
            
    ** [Depends if you've committed a crime or not.]
        ~agatha_trust -= 1
        ...seriously? I'm guessing you don't want me to just *cough* leave you here, sopping wet and freezing, right?
            ***[...]
                ...
            ***[Sorry.]
                ~agatha_trust += 1
                Ah, whatever, dear.  Just maybe cool that kind of talk around everyone else here.

- You should probably take that vest off before we get to the others, some of them probably wouldn't be too pleased to see it.
* [I'll take it off.]
    ~agatha_trust += 1
    ~wearing_uniform = false
    Thanks, dear.
* [I'm keeping it on.]
    ~agatha_trust -= 1
    Suit yourself! Don't say I didn't warn you!
- So I'll introduce you to Ralphie. He's our leader, but he'll pretend he isn't because we don't like that kind of thing here, or some crap like that.
...but yeah, he's our leader.

-> SL1Ralphie_talk_to_ralphie


== function _SL1Ralphie_talk_to_ralphie ==
~ return 1
==SL1Ralphie_talk_to_ralphie==
Ralphie: Hello! I'm Ralphie. How was your trip down here?
{did_player_competently_enter_lvl1_village ==false: 
    Looks like... not great? 
    -> agatha_introduces_to_ralphie
}
* [Good enough.]
    Ah, so those kinda rough guys in the tunnels didn't give you too much trouble? Glad to hear it. -> ralphie_complains_about_tunnel_guys
* [Pretty rough!]
    Oh, was it those rough-looking guys up in the tunnels? -> ralphie_complains_about_tunnel_guys
    

= ralphie_complains_about_tunnel_guys
- I suppose they mean well. They're just trying to guard their home, and the rest of us, but they take it too far sometimes, and end up roughing up nice young ladies such as yourself. It's not right.
Well, anyway, make yourself at home. Been a while since we've had any visitors. What can I get for you, to make up for all that nonsense?

Can I get you something to drink? Some tea? I think we've still got some from the last supply run... yep! 6 months past its use-by, that alright? ..wait, that doesn't make sense, why would tea have a use-by date?
* [No, thanks.]
     Please, don't be shy! You're our guest, of course you can have the best we have to offer! <>->tea 
        
* [Yes, please!]
    ~ralphie_trust += 1
    Wonderful.
    -> tea

= tea
Now, do you take your tea with or without milk?
    * [With]
        Ah, sorry about this, but we don't actually have any...
        -> tea_is_ordered
    * [Without]
         ~ralphie_trust += 1
        Perfect, because we don't have any.
        -> tea_is_ordered
        
= tea_is_ordered
Hey, David, would you be able to get this nice young lady some tea? Thank you very much.

-> what_can_ralphie_do_for_you


= agatha_introduces_to_ralphie
Agatha: Ralphie, this is...
*[Krista.]
    Ralphie: Good to meet you, Krista. 
    ~ ralphie_knows_kristas_name = true
    
*[A regulator from the Department of Transport.]
    -> ralphie_appreciates_directness_about_DOT ->
    
- Agatha:Yeah, she's {agatha_trust > 0: alright.|... yeah.}
<> I'm gonna go now.
Ralphie: Bye, Agatha.
*[Bye!]
    ~agatha_trust += 1
*[...]
    ~agatha_trust-=1

- Hope your trip down here wasn't too rough. No trouble from those kinda intimidating guys up in the tunnels?
    -> ralphie_complains_about_tunnel_guys


=ralphie_appreciates_directness_about_DOT
~ralphie_trust += 1
Ralphie: Ah, direct. That's a good way to be, I appreciate you not wasting my time.
->->

=what_can_ralphie_do_for_you
Ralphie: Now, what can I do for you 
{ralphie_knows_kristas_name:
    <>, Krista?
-else:
    <>... uhh... sorry, do I know your name? I maybe forgot. Getting old, et cetera.
    -> tell_ralphie_name_by_self ->
}
{ralphie_appreciates_directness_about_DOT||wearing_uniform: 
    I 
    {-ralphie_appreciates_directness_about_DOT:
        <> know
    -else:
        {wearing_uniform:
            <> can see 
            ~ralphie_trust -= 1
        }
    
    }
        <> you're from the Department of Transport, but we don't have much transport down here! 
        
}
-> questions_what_can_ralphie_do_for_you


=tell_ralphie_name_by_self
*[It's Krista.]
    ~ralphie_knows_kristas_name = true
    ~ralphie_trust += 1
    Ralphie: Right, how can I help you, Krista?
*[...]
    ~ralphie_trust -= 1
    Ralphie: Alright... well, let me know if you think of anything.
-
->->

==questions_what_can_ralphie_do_for_you==
*[Do you know anything about stolen tapes?]
    Ralphie: Oh, is that about all that nonsense with the people further down? We've been having a lot of trouble with them lately, lots of noise and lots of people marching through the tunnels with boxes of... I think machinery? Guess they're building some new stuff.
    Wish we had the tools to do that... all we do is sit around and complain.
    {
    -not tea_arrives: 
    -> tea_arrives
    -else:
    ->questions_what_can_ralphie_do_for_you
    }
*[Is there a guy down here who wears a weird hat?]
    Ralphie: Oh, you mean Paul? Right, makes sense you'd wanna see him. He lives in the furthest tent, just a little bit outside this little cluster here, but not far.
    {
    -not tea_arrives: 
    -> tea_arrives
    -else:
    ->questions_what_can_ralphie_do_for_you
    }
*   ->
    Ralphie: How's the tea?
    **[Wonderful, thank you.]
        Always nice to have something to warm you up after a long journey.
        ...It's okay to admit that it tastes like death, though.
        -> DONE
    **[Pretty bad.]
        ~ralphie_trust += 1
        That's why I don't drink it! Agatha
        {
        - did_player_competently_enter_lvl1_village:
            <> - lovely woman who lives a few tents down -
        - else:
            <> - lovely lady who rescued you -
        }
        
        <> swears by the stuff, says it's how she stays so spritely *despite being even older than me!* But I can't stomach it. I appreciate your honesty.

Ralphie: Alright well, great to meet you. I suppose you'll be wanting to explore a bit, or go see Paul, so I'll stop boring you with my wittering. See you!
-> hub_character_hub

=tea_arrives
David: Here's your... um... tea... yeah. *sigh*
Ralphie: Thank you, David.
That was a touch performative, don't you think? Just kinda rude, if you ask me.
*[What's his deal?]
    ~ralphie_trust += 1
    Ah, now I've started a controversy. Whoops!
    **[Well, now you have to tell me.]
        ~ralphie_trust += 1
        If you insist, I guess I have no choice. It'd be rude otherwise!
        -> ralphie_complains_about_david
    **[Never mind, then.]
         ~ralphie_trust -= 1
        No, you're supposed to get curious and pressure me to explain what I meant, darn it! Try again!
        ***[...]
             ~ralphie_trust -= 1
             Alright, never mind then.
        ***[Okay, tell me.]
            ~ralphie_trust += 1
            Hooray! ->ralphie_complains_about_david ->
        
*[I guess.]
    ~ralphie_trust-= 1
    You're not a very curious person, are you, {ralphie_knows_kristas_name:Krista}?
    ...
-
->questions_what_can_ralphie_do_for_you

=ralphie_complains_about_david
Ralphie: I suppose he's just been pretty depressed lately. There are some pretty big changes happening around here, and it's been, I'll admit, pretty miserable to be stuck down in the sewer tunnels for the last few months. People want to go home, others want to stay put. I guess it's understandable.
Oh, it feels so naughty to spread gossip! Not like me at all... but it's good to get it out!
Anything else you want to know?
->->

== function _SL1Paul_talk_to_paul ==
~ return 1
==SL1Paul_talk_to_paul==
???: Can I help you?
*[Are you the guy with the hat?]
    ...Who sent you? Intelligence? Police? Military? I thought they gave up on that operation!
    **[Department of Transport?]
        Oh, shit. I didn't say anything, okay?
       * **[My lips are sealed.]
            ~paul_trust +=1
            Very good. You got my trust now. 
            Be careful with it. 
            It's important.
            But it's also fragile.
            Don't betray it.
            Or you'll lose it.
            You got that?
            ***[Yep.]
                Great.
            
        **[What?]
            ~paul_trust -= 1
            ...
- Paul: I'm Paul. I'm undercover here, and they don't suspect a thing. 
Here to sniff out any deviant behaviour.
Terrorism.
Extortion.
Kidnapping.
Smuggling.
Stuff that threatens the core of our city.
...and the Department of Transport too, I guess.
*[Sounds important.]
    You bet it is. I'm the last line of defence between peace and anarchy.
    Doing a pretty good job so far, too. Very peaceful here most of the time.
    The old folks just drink tea all day and the young ones read pamphlets to each other.
    Don't know what's in them, they never ask me to join in on their little book club, but that's probably because I caught them smoking weed once.
    So I did the patriotic thing and burned it all. Felt a little funny afterwards, but it was worth it to protect the city. Guess they didn't like that.
    Other than that, no terrorist activity so far.
-
*[Do you know anything about those tapes that got stolen?]
    I was briefed on that.
    Very important stuff.
    Did some searching.
    Some sniffing around.
    I can be a pretty good detective.
    Searched high and low for 'em.
    Went into everyone's tent when they weren't looking.
    Combed every inch of the place.
    Haven't found 'em yet.
    Maybe you'd have more luck though?
-
*[That's a bit above my pay grade.]
    Depends on what you consider to be your pay.
    I like to think that I get paid in social stability.
    That's more than enough compensation for me.
    Money's just a bonus.
    So what would you prefer to get paid in?
    TERRORISM?!?!
    **[Of course not!]
        {
        -paul_trust > 0:
            ~paul_trust += 1
            I believe you.
            We've got that trust locked in, remember?
            You're like a sister to me.
            A public services sister.
            Don't forget that.
        -else:
            Hmmph. Alright.
        }
        
    **[Seriously?]
        ~paul_trust -= 2
        Deadly. Fucking. Serious.
        The threat's all around us, where you least expect it.
*[If even you can't find them, surely I can't!]
    ~paul_trust += 1
    Ah, good point.
    You've got a good head on your shoulders.
    You'd be a good spy.
    We'd make a good team.
    
- Anyway, this lot are too busy arguing amongst themselves to be stealing tapes.
*[What about?]
- They want to go back up to the city. 
...or some of them do.
And do drugs.
And crime.
That old guy, what's his name? ...Alfie, Alfred, whatever.
Keeps saying that that ancient lady is dying, gotta get care for her.
I bet he means drugs.
They're gonna do drugs in the street.
Then blow up a car.
Mark my words.
Plus, if they go back, that means my mission is done.
Which means that that wall holding everything back from chaos (me, I'm talking about me) is gone.
So, needless to say, I'll be voting to stay.
*[You're voting on it?]
    Yep. We're all gonna vote when that... attractive young lady gets back from her supply run.
    I get a vote because they think I'm one of them.
    But I'm actually not.
    I'm undercover.
    They don't suspect a thing.
    Because I'm so good at staying undercover.
    So yeah, I'm voting no on returning to the city.
    Not sure how the others will vote.
    You should ask them.
    I'd do it myself, but it'd blow my cover. So figure out how everyone's voting and get back to me.
-
*[Ok, I'll do it.]
    {
        -paul_trust > 0:
           Godspeed, soldier.
        -else:
            Well? Get to it!
    }
-> hub_character_hub

== function _SL1Agatha_talk_to_agatha ==
~ return 1
==SL1Agatha_talk_to_agatha==

{(not agatha_conversation_intro): ->agatha_conversation_intro |Hello again, dear.->agatha_conversation_body}

=agatha_conversation_intro
{
    -did_player_competently_enter_lvl1_village:
        Agatha: Hi, dear. I'm Agatha. Hopefully the others that you've met have said nice things about me... otherwise I might have trouble convincing you I'm not a total grouch.
    -else: //we've already met her
        {
            -agatha_trust>0:
                Agatha: Hi dear, good to see you. <>
            -else:
                Agatha: Oh, you again. <>
        }
        {
            -wearing_uniform:
                ~agatha_trust -= 1
                I thought I told you to take that uniform off?
                
        }
    
}
- Agatha: How are you finding the place so far?
*[Nice enough.]
    Yes, I suppose so. Not *cough* much, but it's enough.
    Works well for me, because they don't put me to work.
    I'm just an old lady, so they just let me sit around and drink tea all day.
    ->agatha_conversation_body
*[The tea is bad.]
    ~agatha_trust -= 1
    It's an *acquired taste,* dear. Your palate just maybe isn't refined enough.
    ->agatha_conversation_body

=agatha_conversation_body
~ temp questions_asked = 0
Agatha:
<>{questions_asked==0: Now w|W}hat did you want to talk about?
~ questions_asked +=1
*[Do you know anything about stolen tapes?]
    Tapes? Like... cassette tapes? Dear, it's not 1980 anymore.
    You can listen to music on CDs now. Isn't that neat?
    -> agatha_conversation_body
*{SL1Paul_talk_to_paul} [How do you feel about going back up to the city?]
        Ah. 
        That.
        Well, uh...
        *cough*
        Excuse me, I was holding that one in.
        I don't know why everyone else feels the need to go home just because I can't stay.
        It feels a bit like giving up, to be honest.
        Ralphie won't let me go back alone, though. He keeps talking about money for the doctors, or whatever meaningless rubbish like that.
        I think they should stay. But I'm voting to leave.
        -> agatha_conversation_body
*   ->
    Nothing much.

-
-> hub_character_hub

==lvl1_time_to_vote==
-> DONE