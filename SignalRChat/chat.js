var chat;
var roomName = "main";

$(function () {
    // Declare a proxy to reference the hub.
    chat = $.connection.chatHub;
    // Create a function that the hub can call to broadcast messages.
    chat.client.addChatMessage = function (name, message) {
        // Html encode display name and message.
        var encodedName = $('<div />').text(name).html();
        var encodedMsg = $('<div />').text(message).html();
        // Add the message to the page.
        $('#discussion').append('<li><strong>' + encodedName
            + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');

        var objDiv = document.getElementById("chatDiv");
        objDiv.scrollTop = objDiv.scrollHeight;
    };



    // Set initial focus to message input box.
    $('#message').focus();

    // Start the connection.
    $.connection.hub.start().done(function () {
        // Get the room name and store it
        roomName = prompt("Please enter chat name", "main");
        if (!roomName) {
            roomName = "main";
        }

        // Get the user name and store it to prepend to messages.
        $('#displayname').val(prompt('Enter your name:', ''));
        if ($('#displayname').val() == "") {
            $('#displayname').val("guest " + Math.floor(Math.random() * 100000));
        }

        $('#welcome').append("<h4>Welcome " + $('#displayname').val() + "</h4><br>Your connectionId is = " + $.connection.hub.id);

        chat.server.joinRoom(roomName, $('#displayname').val());
        chat.server.getGroupList().done(fillGroupList);

        $('#sendmessage').click(function () {
            // Call the Send method on the hub.
            if ($('#message').val() != "") {
                chat.server.send(roomName, $('#displayname').val(), $('#message').val());
            }
            // Clear text box and reset focus for next comment.
            $('#message').val('').focus();
        });





    });

    chat.client.broadcastNewGroupCreated = fillGroupList;
    chat.client.onlineListChanged = fillOnlineList;


    $("#message").keyup(function (event) {
        if (event.keyCode === 13) {
            $("#sendmessage").click();
        }
    });





});

function fillGroupList(roomList) {
    $('#rooms').empty();
    for (i = 0; i < roomList.length; i++) {
        if (roomList[i] == roomName) {
            $('#rooms').append('<li id="' + roomList[i] + '"><strong>' + roomList[i] + '</strong></li>');
        }
        else {
            $('#rooms').append('<li id="' + roomList[i] + '">' + roomList[i] + '</li>');
        }
    }

    var roomsDiv = document.getElementById("rooms");
    var groups = roomsDiv.getElementsByTagName('li');
    var changeGroupEventListener = function (e) {
        changeGroup(e.currentTarget.id);
    };

    for (var i = 0; i < groups.length; i++) {
        var group = groups[i];
        group.addEventListener("click", changeGroupEventListener);
    }
}

function changeGroup(newRoomName) {
    chat.server.leaveRoom(roomName, $('#displayname').val());
    chat.server.joinRoom(newRoomName, $('#displayname').val());
    roomName = newRoomName;
    chat.server.getGroupList().done(fillGroupList);
    $('#discussion').empty();
    fillOnlineList(newRoomName);
}

function fillOnlineList(newRoomName) {
    chat.server.getOnlineList(newRoomName).done(function (onlines) {
        $('#online').empty();
        for (i = 0; i < onlines.length; i++) {
            $('#online').append('<li>' + onlines[i].Name + '</li>');
        }
    });
}

