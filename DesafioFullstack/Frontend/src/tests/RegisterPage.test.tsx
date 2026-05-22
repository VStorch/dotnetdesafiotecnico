import { render, screen } from "@testing-library/react";
import { BrowserRouter } from "react-router-dom";
import Register from "../pages/Register";
import { AuthProvider } from "../context/AuthContext";
import { expect, test } from "vitest";

function renderRegister() {
  return render(
    <AuthProvider>
      <BrowserRouter>
        <Register />
      </BrowserRouter>
    </AuthProvider>,
  );
}

test("renders register form", () => {
  renderRegister();

  expect(screen.getByPlaceholderText("Email")).toBeInTheDocument();
  expect(screen.getByPlaceholderText("Password")).toBeInTheDocument();
  expect(screen.getByPlaceholderText("Confirm Password")).toBeInTheDocument();
});
