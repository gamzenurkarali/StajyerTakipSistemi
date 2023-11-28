"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/ConnectedHub").build();

connection.on("ReceiveMessage", function (senderGuid, receiverGuid, message, timestamp) {
    var date = new Date(timestamp * 1000); // Unix zaman damgasını milisaniyeye dönüştür
    var formattedDate = date.toLocaleDateString();
    var formattedTime = date.toLocaleTimeString(undefined, { hour: '2-digit', minute: '2-digit', hour12: false });

    var li = document.createElement("li");

    // Mevcut kullanıcının kimliğini al
    var currentUserGuid = document.getElementById("senderInput").value; // Bu, kullanıcının kimliğini almanız gereken yere gelecek

    if (senderGuid === currentUserGuid) {
        li.classList.add("li-sender");
    } else if (receiverGuid === currentUserGuid) {
        li.classList.add("li-receiver");
    }

    var balloonDiv = document.createElement("div"); 
    balloonDiv.innerHTML = message;
    if (senderGuid === currentUserGuid) {
        balloonDiv.classList.add("sender-balloon");
    } else if (receiverGuid === currentUserGuid) {
        balloonDiv.classList.add("receiver-balloon");
    }
    var timeDiv = document.createElement("div");
    timeDiv.classList.add("message-time");
    var messageTimeSpan = document.createElement("span");
    messageTimeSpan.textContent = formattedTime;
    timeDiv.appendChild(messageTimeSpan);

    li.appendChild(balloonDiv);
    li.appendChild(timeDiv);

    var existingDateDiv = document.querySelector(`div[data-date="${formattedDate}"]`);
    if (!existingDateDiv) {
        // Eğer bu tarih için bir div yoksa, oluştur ve messagesList'e ekle
        var dateDiv = document.createElement("div");
        dateDiv.style.display = 'flex';
        dateDiv.style.alignSelf = 'center';

        var span = document.createElement("span");
        span.style.background = 'lightgray';
        span.style.paddingInline = '1rem';
        span.style.borderRadius = '11px';
        span.innerText = formattedDate;

        dateDiv.appendChild(span);
        dateDiv.setAttribute("data-date", formattedDate);

        document.getElementById("messagesList").appendChild(dateDiv);
        var noMessagesAvailable = document.getElementById("noMessagesAvailable");
        if (noMessagesAvailable) {
            noMessagesAvailable.remove();
        }
    }

    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});
 
document.getElementById("sendButton").addEventListener("click", function (event) {
    event.preventDefault();  

    var senderGuid = document.getElementById("senderInput").value;
    var receiverGuid = document.getElementById("receiverInput").value;
    var message = document.getElementById("messageInput").value;

    if (senderGuid && receiverGuid && message.trim() !== "") {
         
        connection.invoke("SendMessage", senderGuid, receiverGuid, message).then(function () {
             
            $('#messageInput').data('emojioneArea').setText(''); 
        }).catch(function (err) {
            console.error(err.toString());
             
        });
         
    }
});

  