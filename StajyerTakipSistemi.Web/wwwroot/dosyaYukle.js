 document.getElementById("dosyaYukleButton").addEventListener("click", function () {
            document.getElementById("dosyaYukleInput").click();
        });

        document.getElementById("dosyaYukleInput").addEventListener("change", function () {
            var secilenDosya = document.getElementById("dosyaYukleInput").files[0];
            var secilenDosyaAdi = secilenDosya.name;

            var secilenDosyaDiv = document.getElementById("secilenDosya");
            secilenDosyaDiv.innerHTML = "Seçilen Dosya: " + secilenDosyaAdi;
            if (secilenDosya) { 
                messageText.disabled = true;
                messageText.removeAttribute("placeholder");
            }
        });