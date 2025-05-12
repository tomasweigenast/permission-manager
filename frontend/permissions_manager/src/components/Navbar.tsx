import { AppBar, Toolbar, Typography, Container, Box } from "@mui/material";
import { Link } from "react-router-dom";
import PermissionIcon from "@mui/icons-material/VpnKey";

export default function Navbar() {
  return (
    <AppBar position="sticky" color="primary" elevation={1}>
      <Container maxWidth="md" disableGutters sx={{ width: "100%" }}>
        <Toolbar>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              color: "white",
              textDecoration: "none",
              margin: "0 auto",
              width: "100%",
              maxWidth: "md",
            }}
            component={Link}
            to="/"
          >
            <PermissionIcon sx={{ mr: 1 }} />
            <Typography
              variant="h6"
              component="div"
              sx={{ fontWeight: "bold" }}
            >
              Permission Manager
            </Typography>
          </Box>
        </Toolbar>
      </Container>
    </AppBar>
  );
}
