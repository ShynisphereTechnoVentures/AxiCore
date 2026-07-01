window.auth ={

    setAuthData: function (token, role, fullName){

        localStorage.setItem(
            "authToken",
            token);

        localStorage.setItem(
            "userRole",
            role);

        localStorage.setItem(
            "fullName",
            fullName);
    },

    getToken: function (){
        return localStorage.getItem(
            "authToken");
    },

    getRole: function (){
        return localStorage.getItem(
            "userRole");
    },

    getFullName: function (){
        return localStorage.getItem(
            "fullName");
    },

    logout: function (){

        localStorage.removeItem(
            "authToken");

        localStorage.removeItem(
            "userRole");

        localStorage.removeItem(
            "fullName");
    }
};
