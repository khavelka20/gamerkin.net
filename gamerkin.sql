-- phpMyAdmin SQL Dump
-- version 4.5.5.1
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1
-- Generation Time: Oct 16, 2016 at 06:30 PM
-- Server version: 5.7.11
-- PHP Version: 7.0.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `gamerkin`
--

DELIMITER $$
--
-- Procedures
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `fix_outliers` (IN `gamerId` INT)  BEGIN
UPDATE gamerkin.gamer_games 
SET 
    include = 0
WHERE
    gamerkin.gamer_games.gamer_id = gamerId
        AND gamerkin.gamer_games.game_id IN (SELECT 
            outliers.game_id
        FROM
            (SELECT 
                g.time_played,
                    g.game_id,
                    (g.time_played / (SELECT 
                            STD(g.time_played)
                        FROM
                            gamerkin.gamer_games g
                        WHERE
                            g.gamer_id = gamerId
                        GROUP BY g.gamer_id)) AS standard_dev_time
            FROM
                gamerkin.gamer_games g
            WHERE
                g.gamer_id = gamerId
            HAVING standard_dev_time > 3) AS outliers);
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_get_recommended_games` (IN `gamerId` INT, IN `minRating` INT, IN `limitTo` INT)  BEGIN
set @temp_id = 0;

SELECT 
    game_id,predicted_score, @temp_id:=@temp_id+1 AS id, steam_id, name, logo, steam_user_rating,
    metacritic_rating, description, type , created_at, updated_at, steam_user_rating_value, developer
FROM
    (SELECT 
		MAX(steam_user_rating) as steam_user_rating,
        MAX(metacritic_rating) as metacritic_rating,
        MAX(description) as description,
        MAX(type) as type,
        MAX(created_at) as created_at,
        MAX(updated_at) as updated_at,
        MAX(developer) as developer,
		MAX(g.logo) as logo,
		MAX(g.name) as name,
		MAX(g.steam_id) as steam_id,
        MAX(g.steam_user_rating_value) AS steam_user_rating_value,
            MAX(game.game_id) AS game_id,
            SUM(gamer.genre_preference * game.genre_score) / (SQRT((SELECT 
                    SUM(POW(gamerkin.gamer_genre_scores.genre_preference, 2))
                FROM
                    gamerkin.gamer_genre_scores
                WHERE
                    gamerkin.gamer_genre_scores.gamer_id = gamerId
                GROUP BY gamer_id)) * SQRT(SUM(POW(game.genre_score, 2)))) AS predicted_score
    FROM
        gamer_genre_scores gamer
    JOIN game_genre_scores game ON gamer.genre_id = game.genre_id
    JOIN gamerkin.games g ON game.game_id = g.id
    WHERE
        gamer.gamer_id = gamerId
    GROUP BY game.game_id) AS game_prediction
WHERE
    game_id NOT IN (SELECT 
            gg.game_id
        FROM
            gamerkin.gamer_games gg
        WHERE
            gg.gamer_id = gamerId)
AND steam_user_rating_value > minRating
ORDER BY predicted_score desc, steam_user_rating_value desc
LIMIT limitTo;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `gamers`
--

CREATE TABLE `gamers` (
  `id` int(10) NOT NULL,
  `steam_id` bigint(20) NOT NULL,
  `avatar` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `avatar_medium` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `created_at` date NOT NULL,
  `updated_at` date NOT NULL,
  `name` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `gamer_games`
--

CREATE TABLE `gamer_games` (
  `id` int(10) UNSIGNED NOT NULL,
  `gamer_id` int(10) NOT NULL,
  `game_id` int(10) NOT NULL,
  `rating` int(11) DEFAULT NULL,
  `created_at` date NOT NULL,
  `updated_at` date NOT NULL,
  `time_played` int(11) DEFAULT NULL,
  `include` int(11) DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Stand-in structure for view `gamer_genre_scores`
--
CREATE TABLE `gamer_genre_scores` (
`gamer_id` int(10)
,`genre_id` int(10)
,`genre_preference` double(17,0)
);

-- --------------------------------------------------------

--
-- Table structure for table `gamer_genre_skew`
--

CREATE TABLE `gamer_genre_skew` (
  `id` int(11) NOT NULL,
  `gamer_id` int(11) DEFAULT NULL,
  `genre_id` int(11) DEFAULT NULL,
  `skew` int(11) DEFAULT NULL,
  `created_at` date DEFAULT NULL,
  `updated_at` date DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `games`
--

CREATE TABLE `games` (
  `id` int(10) NOT NULL,
  `steam_id` int(11) NOT NULL,
  `name` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `logo` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `steam_user_rating` varchar(255) DEFAULT NULL,
  `metacritic_rating` int(11) DEFAULT NULL,
  `description` text CHARACTER SET utf8 COLLATE utf8_unicode_ci,
  `type` varchar(11) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `created_at` date NOT NULL,
  `updated_at` date NOT NULL,
  `steam_user_rating_value` int(11) DEFAULT NULL,
  `developer` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `predicted_score` decimal(10,10) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `game_assets`
--

CREATE TABLE `game_assets` (
  `id` int(11) NOT NULL,
  `type` varchar(255) DEFAULT NULL,
  `status` varchar(255) DEFAULT NULL,
  `game_id` int(10) NOT NULL,
  `created_at` date DEFAULT NULL,
  `updated_at` date DEFAULT NULL,
  `youtube_id` varchar(45) DEFAULT NULL,
  `clip_duration` int(11) DEFAULT NULL,
  `thumb_url` varchar(255) DEFAULT NULL,
  `full_url` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `game_genre`
--

CREATE TABLE `game_genre` (
  `id` int(10) UNSIGNED NOT NULL,
  `game_id` int(10) NOT NULL,
  `created_at` date NOT NULL,
  `updated_at` date NOT NULL,
  `rank` int(11) NOT NULL,
  `genre_id` int(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Stand-in structure for view `game_genre_scores`
--
CREATE TABLE `game_genre_scores` (
`game_name` varchar(255)
,`game_id` int(10)
,`genre_id` int(10)
,`genre_name` varchar(255)
,`genre_score` varchar(58)
);

-- --------------------------------------------------------

--
-- Table structure for table `game_platforms`
--

CREATE TABLE `game_platforms` (
  `id` int(11) NOT NULL,
  `game_id` int(10) DEFAULT NULL,
  `name` varchar(255) DEFAULT NULL,
  `created_at` date DEFAULT NULL,
  `updated_at` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `game_prices`
--

CREATE TABLE `game_prices` (
  `id` int(10) UNSIGNED NOT NULL,
  `game_id` int(10) DEFAULT NULL,
  `source` varchar(255) DEFAULT NULL,
  `created_at` date DEFAULT NULL,
  `updated_at` date DEFAULT NULL,
  `price` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `game_requirements`
--

CREATE TABLE `game_requirements` (
  `id` int(11) NOT NULL,
  `platform` varchar(255) DEFAULT NULL,
  `description` text,
  `created_at` date DEFAULT NULL,
  `updated_at` date DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `genres`
--

CREATE TABLE `genres` (
  `id` int(10) NOT NULL,
  `name` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `created_at` date NOT NULL,
  `updated_at` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `migrations`
--

CREATE TABLE `migrations` (
  `migration` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `batch` int(11) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `steam_rating_derived_score`
--

CREATE TABLE `steam_rating_derived_score` (
  `steam_user_rating` varchar(255) DEFAULT NULL,
  `id` int(11) NOT NULL,
  `score` int(11) DEFAULT NULL,
  `skew` decimal(10,1) DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure for view `gamer_genre_scores`
--
DROP TABLE IF EXISTS `gamer_genre_scores`;
-- in use(#1046 - No database selected)

-- --------------------------------------------------------

--
-- Structure for view `game_genre_scores`
--
DROP TABLE IF EXISTS `game_genre_scores`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `game_genre_scores`  AS  select `g`.`name` AS `game_name`,`g`.`id` AS `game_id`,`ge`.`id` AS `genre_id`,`ge`.`name` AS `genre_name`,format((((11 - `gg`.`rank`) / 10) * 50),0) AS `genre_score` from ((`games` `g` left join `game_genre` `gg` on((`g`.`id` = `gg`.`game_id`))) join `genres` `ge` on((`gg`.`genre_id` = `ge`.`id`))) where ((`g`.`type` = 'Game') and (`gg`.`rank` <= 10)) ;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `gamers`
--
ALTER TABLE `gamers`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `gamer_games`
--
ALTER TABLE `gamer_games`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_gamer_games_gamer_idx` (`gamer_id`),
  ADD KEY `fk_gamer_games_game_idx` (`game_id`);

--
-- Indexes for table `gamer_genre_skew`
--
ALTER TABLE `gamer_genre_skew`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `games`
--
ALTER TABLE `games`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `game_assets`
--
ALTER TABLE `game_assets`
  ADD PRIMARY KEY (`id`),
  ADD KEY `GAME` (`game_id`);

--
-- Indexes for table `game_genre`
--
ALTER TABLE `game_genre`
  ADD PRIMARY KEY (`id`),
  ADD KEY `genre_id_game_id` (`game_id`,`genre_id`),
  ADD KEY `fk_game_genres_genre_idx` (`genre_id`);

--
-- Indexes for table `game_platforms`
--
ALTER TABLE `game_platforms`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_game_platforms_game_idx` (`game_id`);

--
-- Indexes for table `game_prices`
--
ALTER TABLE `game_prices`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_games_game_prices_idx` (`game_id`);

--
-- Indexes for table `game_requirements`
--
ALTER TABLE `game_requirements`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `genres`
--
ALTER TABLE `genres`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `steam_rating_derived_score`
--
ALTER TABLE `steam_rating_derived_score`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `gamers`
--
ALTER TABLE `gamers`
  MODIFY `id` int(10) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;
--
-- AUTO_INCREMENT for table `gamer_games`
--
ALTER TABLE `gamer_games`
  MODIFY `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=791;
--
-- AUTO_INCREMENT for table `gamer_genre_skew`
--
ALTER TABLE `gamer_genre_skew`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;
--
-- AUTO_INCREMENT for table `games`
--
ALTER TABLE `games`
  MODIFY `id` int(10) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26930;
--
-- AUTO_INCREMENT for table `game_assets`
--
ALTER TABLE `game_assets`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=100908;
--
-- AUTO_INCREMENT for table `game_genre`
--
ALTER TABLE `game_genre`
  MODIFY `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=66862;
--
-- AUTO_INCREMENT for table `game_platforms`
--
ALTER TABLE `game_platforms`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13840;
--
-- AUTO_INCREMENT for table `game_prices`
--
ALTER TABLE `game_prices`
  MODIFY `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7862;
--
-- AUTO_INCREMENT for table `game_requirements`
--
ALTER TABLE `game_requirements`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `genres`
--
ALTER TABLE `genres`
  MODIFY `id` int(10) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=369;
--
-- AUTO_INCREMENT for table `steam_rating_derived_score`
--
ALTER TABLE `steam_rating_derived_score`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;
--
-- Constraints for dumped tables
--

--
-- Constraints for table `gamer_games`
--
ALTER TABLE `gamer_games`
  ADD CONSTRAINT `fk_gamer_games_game` FOREIGN KEY (`game_id`) REFERENCES `games` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_gamer_games_gamer` FOREIGN KEY (`gamer_id`) REFERENCES `gamers` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `game_assets`
--
ALTER TABLE `game_assets`
  ADD CONSTRAINT `fk_game_assets_games` FOREIGN KEY (`game_id`) REFERENCES `games` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Constraints for table `game_genre`
--
ALTER TABLE `game_genre`
  ADD CONSTRAINT `fk_game_genres_game` FOREIGN KEY (`game_id`) REFERENCES `games` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_game_genres_genre` FOREIGN KEY (`genre_id`) REFERENCES `genres` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `game_platforms`
--
ALTER TABLE `game_platforms`
  ADD CONSTRAINT `fk_game_platforms_game` FOREIGN KEY (`game_id`) REFERENCES `games` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `game_prices`
--
ALTER TABLE `game_prices`
  ADD CONSTRAINT `fk_game_prices_game` FOREIGN KEY (`game_id`) REFERENCES `games` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
