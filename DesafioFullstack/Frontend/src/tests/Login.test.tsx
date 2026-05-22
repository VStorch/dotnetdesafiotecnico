import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { BrowserRouter } from "react-router-dom";
import { test, expect } from "vitest";

import Login from "../pages/Login";
import { AuthProvider } from "../context/AuthContext";

function renderLogin() {
  return render(
    <AuthProvider>
      <BrowserRouter>
        <Login />
      </BrowserRouter>
    </AuthProvider>,
  );
}

test("renders login form", () => {
  renderLogin();

  expect(screen.getByPlaceholderText("Email")).toBeInTheDocument();
  expect(screen.getByPlaceholderText("Password")).toBeInTheDocument();
});

test("shows error when fields are empty", async () => {
  renderLogin();

  const button = screen.getByRole("button", { name: /login/i });
  await userEvent.click(button);

  expect(await screen.findByText("Invalid credentials")).toBeInTheDocument();
});
