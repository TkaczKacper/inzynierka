"use client";

import { useState } from "react";

const LoginForm = () => {
   const [username, setUsername] = useState("");
   const [password, setPassword] = useState("");
   console.log(username, password);

   const loginHandler = () => {};

   return (
      <form onSubmit={loginHandler}>
         <label htmlFor="username">Username: </label>
         <input
            type="text"
            id="username"
            name="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
         />

         <label htmlFor="password">Password: </label>
         <input
            type="password"
            id="password"
            name="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
         />
         <button>Login</button>
      </form>
   );
};

export default LoginForm;
