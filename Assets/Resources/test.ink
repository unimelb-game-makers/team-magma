VAR secret_var = 0

== function _TestCube_find_cube ==
~ return 1

== TestCube_find_cube ==
# speaker: ???
You've found a mysterious glowing cube.
+ [?]
    ~ secret_var += 1
    The cube hums quietly but doesn't respond.
    
    ** [Nevermind.] // Nested choice
        You decide to leave it alone.
        -> move_on
        
    ++ [Hmm. Weird.] // Another nested choice
        This world is already weird enough. You decide to ignore it.
        ~ secret_var += 1
        -> move_on
        
* [What is this?] // Alternative main choice
    Nothing happens.
    ** [What is this???]
    ** (tilt) You tilt your head[] at the cube.
    -- Nothing happens.
    *** [examine closer]
    -> examine_closer
    
* [Touch it] // Shows conditional content
    { 
        - secret_var < 2:
            Your hand passes right through it! It's just a hologram!
            -> hologram_reveal
        - else:
            The cube solidifies as you touch it. It feels warm.
            -> cube_activated
    }

= examine_closer
{
    - tilt: As you examine the cube, you notice faint symbols:
        ** [Try to read them]
            # speaker: Cube
            Press F to pay respects
            *** [Press F]
                # speaker: ???
                The cube dissolves into light!
                ~ secret_var = 99
                -> cube_activated
            *** [Don't press F]
                # speaker: ???
                The cube seems disappointed.
                -> move_on
        ** [Back away]
            -> move_on
    - else:
        Nothing happens again!
        -> DONE
}

= hologram_reveal
The holographic cube flickers and displays a message.
* [Try to read the message]
    This was a test. {secret_var} attempts recorded.
    -> move_on

= cube_activated
The cube transforms into { 
    - secret_var > 5: <> a swirling portal // <> prevent new lines
    - else: <> a small key
}

* [Take it]
    You acquired { 
        - secret_var > 5: <> the Portal Cube!
        - else: <> a Mysterious Key
    }
    -> DONE

= move_on
# speaker: ???
You leave the cube behind.
-> DONE

/* 
TUTORIAL NOTES:

1. VARIABLES:
   - Declare with VAR at top
   - Modify with ~ (e.g., ~secret_var += 1)
   - Use in conditions: {secret_var > 5}

2. CHOICES:
   - * [Option] for main choices
   - ** [Option] for nested choices
   - {condition: content} for conditional text
   - + [Option] for choices that always exist

3. DIVERTS:
   - -> label_name jumps to that section
   - -> DONE ends the conversation

4. TEXT VARIATIONS:
   - { 
       - option1: text1 
       - option2: text2
     }
*/