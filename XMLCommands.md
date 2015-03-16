#XML Commands

# Introduction #

The client and server communication via xml ;)


## Chat ##
### chat (from server) ###
```
<x>
     <type>chat</type>
     <admin>1</admin>                                (if applicable)
     <from>sender_name</from>
     <to>recipient_name</to>
     <channel>channel_name</channel>
     <message>message_text</message>
</x>
```
### chat (to server) ###
```
<x>
     <type>chat</type>
     <to>recipient_name</to>
     <channel>channel_name</channel>
     <message>message_text</message>
</x>
```


## Tasks (from server) ##
```
<x>
     <type>usertask</task>
     <id>id.ToString</id>
     <from_unit>from_unit.ToString()</from_unit>
     <from_number>from_number.ToString()</from_number>
     <to_unit>to_unit.ToString()</to_unit>
     <to_number>to_number.ToString()</to_number>
     <to_unit>to_unit.ToString()</to_unit>
     <task_type>tasktype.ToString()</task_type>
     <task_content>taskcontent</task_content>
     <creation_date>creation_date.ToString()</creation_date>
     <task_type>tasktype.ToString ()</task_type>
</x>
```