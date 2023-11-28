 
    function validateForm() {
        var firstName = document.getElementById("FirstName").value;
        var lastName = document.getElementById("LastName").value;
        var email = document.getElementById("Email").value;
        var phoneNumber = document.getElementById("PhoneNumber").value;
        var birthDate = document.getElementById("BirthDate").value;
        var address = document.getElementById("Address").value;
        var desiredField = document.getElementById("DesiredField").value;
        var explanation = document.getElementById("Explanation").value;
         
        if (firstName === "" || lastName === "" || email === "" || phoneNumber === "" || birthDate === "" || address === "" || desiredField === "" || explanation === "") {
            alert("Tüm alanları doldurunuz.");
             return false;
        }

        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            alert("Geçerli bir e-posta adresi giriniz.");
            return false;
        }

        // Diğer format kontrolü ve kuralları ekleyebilirsiniz

    return true;
    }
     
