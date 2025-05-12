import { BrowserRouter, Routes, Route } from "react-router-dom";
import { ThemeProvider } from "@mui/material/styles";
import { CssBaseline, Box, Container } from "@mui/material";
import theme from "./theme";
import PermissionList from "./components/PermissionList";
import PermissionForm from "./components/PermissionForm";
import Navbar from "./components/Navbar";

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <BrowserRouter>
        <Box
          sx={{
            minHeight: "100vh",
            backgroundColor: "background.default",
            display: "flex",
            flexDirection: "column",
          }}
        >
          <Navbar />
          <Container
            maxWidth="md"
            sx={{
              flexGrow: 1,
              display: "flex",
              flexDirection: "column",
              justifyContent: "flex-start",
              alignItems: "center",
              py: 4,
            }}
          >
            <Box sx={{ width: "100%" }}>
              <Routes>
                <Route path="/" element={<PermissionList />} />
                <Route path="/new" element={<PermissionForm />} />
              </Routes>
            </Box>
          </Container>
        </Box>
      </BrowserRouter>
    </ThemeProvider>
  );
}

export default App;
